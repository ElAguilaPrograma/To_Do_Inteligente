import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmDeletetaskDialogComponent } from './confirm-deletetask-dialog.component';

describe('ConfirmDeletetaskDialogComponent', () => {
  let component: ConfirmDeletetaskDialogComponent;
  let fixture: ComponentFixture<ConfirmDeletetaskDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConfirmDeletetaskDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfirmDeletetaskDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
